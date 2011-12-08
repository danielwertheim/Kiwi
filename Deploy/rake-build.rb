#--------------------------------------
# Dependencies
#--------------------------------------
require 'albacore'
#--------------------------------------
# Debug
#--------------------------------------
#ENV.each {|key, value| puts "#{key} = #{value}" }
#--------------------------------------
# Environment vars
#--------------------------------------
@env_solutionname = 'Kiwi'
@env_projectnameKiwiMarkdown = 'Kiwi.Markdown'
@env_solutionfolderpath = "../Source"
@env_buildversion = "0.7.1" + (ENV['env_buildnumber'].to_s.empty? ? "" : ".#{ENV['env_buildnumber'].to_s}")
@env_buildconfigname = ENV['env_buildconfigname'].to_s.empty? ? "Release" : ENV['env_buildconfigname'].to_s
@env_buildname = "#{@env_solutionname}-v#{@env_buildversion}-#{@env_buildconfigname}"
@env_buildfolderpath = "#{ENV['env_buildfolderpath']}Builds/#{@env_buildname}"
@env_outputfolderpath = @env_buildfolderpath
#--------------------------------------
#optional if no remote nuget actions should be performed
@env_nugetPublishApiKey = ENV['env_nugetPublishApiKey']
@env_nugetPublishUrl = ENV['env_nugetPublishUrl']
@env_nugetSourceUrl = ENV['env_nugetSourceUrl']
#--------------------------------------
# Reusable vars
#--------------------------------------
kiwiMarkdownOutputPath = "#{@env_outputfolderpath}/#{@env_projectnameKiwiMarkdown}"
#--------------------------------------
# Albacore flow controlling tasks
#--------------------------------------
task :ci => [:buildIt, :copyIt, :testIt, :zipIt, :packIt, :cleanItUp, :publishIt]

task :local => [:buildIt, :copyIt, :testIt, :zipIt, :packIt, :cleanItUp]
#--------------------------------------
task :testIt => [:unittests, :specifications]

task :zipIt => [:zipKiwiMarkdown]

task :packIt => [:packKiwiMarkdownNuGet]

task :publishIt => [:publishKiwiMarkDownNuGet]
#--------------------------------------
# Albacore tasks
#--------------------------------------
assemblyinfo :versionIt do |asm|
	sharedAssemblyInfoPath = "#{@env_solutionfolderpath}/SharedAssemblyInfo.cs"

	asm.input_file = sharedAssemblyInfoPath
	asm.output_file = sharedAssemblyInfoPath
	asm.version = @env_buildversion
	asm.file_version = @env_buildversion  
end

task :ensureCleanBuildFolder do
	FileUtils.rm_rf(@env_buildfolderpath)
	FileUtils.mkdir_p(@env_outputfolderpath)
end

task :cleanItUp do
	FileUtils.rm_rf(kiwiMarkdownOutputPath)
end

msbuild :buildIt => [:ensureCleanBuildFolder, :versionIt] do |msb|
	msb.properties :configuration => @env_buildconfigname
	msb.targets :Clean, :Build
	msb.solution = "#{@env_solutionfolderpath}/#{@env_solutionname}.sln"
end

task :copyIt do
	FileUtils.mkdir_p(kiwiMarkdownOutputPath)
	FileUtils.cp_r(FileList["#{@env_solutionfolderpath}/Projects/#{@env_projectnameKiwiMarkdown}/bin/#{@env_buildconfigname}/**"], kiwiMarkdownOutputPath)
end

nunit :unittests do |nunit|
	nunit.command = "#{@env_solutionfolderpath}/packages/NUnit.2.5.10.11092/tools/nunit-console.exe"
	nunit.options "/framework=v4.0.30319","/xml=#{@env_buildfolderpath}/NUnit-Report-#{@env_solutionname}-UnitTests.xml"
	nunit.assemblies FileList["#{@env_solutionfolderpath}/Tests/#{@env_solutionname}.**UnitTests/bin/#{@env_buildconfigname}/#{@env_solutionname}.**UnitTests.dll"]
end

mspec :specifications do |mspec|
	mspec.command = "#{@env_solutionfolderpath}/packages/Machine.Specifications.0.5.0.0/tools/mspec-clr4.exe"
	mspec.options "--html #{@env_buildfolderpath}/MSpec-Report-#{@env_solutionname}.html"
	mspec.assemblies FileList["#{@env_solutionfolderpath}/Tests/#{@env_solutionname}.**Specifications/bin/#{@env_buildconfigname}/#{@env_solutionname}.**Specifications.dll"]
end

zip :zipKiwiMarkdown do |zip|
	zip.directories_to_zip kiwiMarkdownOutputPath
	zip.output_file = "#{@env_buildname}-#{@env_projectnameKiwiMarkdownAdmin}.zip"
	zip.output_path = @env_buildfolderpath
end

exec :packKiwiMarkdownNuGet do |cmd|
	cmd.command = "NuGet.exe"
	cmd.parameters = "pack #{@env_projectnameKiwiMarkdown}.nuspec -version #{@env_buildversion} -basepath #{kiwiMarkdownOutputPath} -outputdirectory #{@env_buildfolderpath}"
end

exec :publishKiwiMarkDownNuGet do |cmd|
	cmd.command = "NuGet.exe"
	cmd.parameters = "push #{@env_buildfolderpath}/#{@env_projectnameKiwiMarkdown}.#{@env_buildversion}.nupkg #{@env_nugetPublishApiKey} -src #{@env_nugetPublishUrl}"
end