#Template v1.0.0
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
@env_solutionfolderpath = "../Source"

@env_projectnameKiwiMarkdown = 'Kiwi.Markdown'
@env_projectnameKiwiMvc3 = 'Kiwi.Mvc3'

@env_buildfolderpath = 'build'
@env_version = "0.8.2"
@env_buildversion = @env_version + (ENV['env_buildnumber'].to_s.empty? ? "" : ".#{ENV['env_buildnumber'].to_s}")
@env_buildconfigname = ENV['env_buildconfigname'].to_s.empty? ? "Release" : ENV['env_buildconfigname'].to_s
@env_buildnameKiwiMarkdown = "#{@env_projectnameKiwiMarkdown}-v#{@env_buildversion}-#{@env_buildconfigname}"
@env_buildnameKiwiMvc3 = "#{@env_projectnameKiwiMvc3}-v#{@env_buildversion}-#{@env_buildconfigname}"
#--------------------------------------
# Reusable vars
#--------------------------------------
kiwiMarkdownOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameKiwiMarkdown}"
kiwiMvc3OutputPath = "#{@env_buildfolderpath}/#{@env_projectnameKiwiMvc3}"
#--------------------------------------
# Albacore flow controlling tasks
#--------------------------------------
task :ci => [:buildIt, :copyKiwiMarkdown, :testIt, :zipIt, :packIt]

task :local => [:buildIt, :copyKiwiMarkdown, :testIt, :zipIt, :packIt]
#--------------------------------------
task :testIt => [:unittests]

task :zipIt => [:zipKiwiMarkdown, :zipKiwiMvc3]

task :packIt => [:packKiwiMarkdownNuGet, :packKiwiMvc3NuGet]
#--------------------------------------
# Albacore tasks
#--------------------------------------
assemblyinfo :versionIt do |asm|
	sharedAssemblyInfoPath = "#{@env_solutionfolderpath}/SharedAssemblyInfo.cs"

	asm.input_file = sharedAssemblyInfoPath
	asm.output_file = sharedAssemblyInfoPath
	asm.version = @env_version
	asm.file_version = @env_buildversion
end

task :ensureCleanBuildFolder do
	FileUtils.rm_rf(@env_buildfolderpath)
	FileUtils.mkdir_p(@env_buildfolderpath)
end

msbuild :buildIt => [:ensureCleanBuildFolder, :versionIt] do |msb|
	msb.properties :configuration => @env_buildconfigname
	msb.targets :Clean, :Build
	msb.solution = "#{@env_solutionfolderpath}/#{@env_solutionname}.sln"
end

task :copyKiwiMarkdown do
	FileUtils.mkdir_p(kiwiMarkdownOutputPath)
	FileUtils.cp_r(FileList["#{@env_solutionfolderpath}/Projects/#{@env_projectnameKiwiMarkdown}/bin/#{@env_buildconfigname}/**"], kiwiMarkdownOutputPath)
end

nunit :unittests do |nunit|
	nunit.command = "#{@env_solutionfolderpath}/packages/NUnit.2.5.10.11092/tools/nunit-console.exe"
	nunit.options "/framework=v4.0.30319","/xml=#{@env_buildfolderpath}/NUnit-Report-#{@env_solutionname}-UnitTests.xml"
	nunit.assemblies FileList["#{@env_solutionfolderpath}/Tests/#{@env_solutionname}.**UnitTests/bin/#{@env_buildconfigname}/#{@env_solutionname}.**UnitTests.dll"]
end

zip :zipKiwiMarkdown do |zip|
	zip.directories_to_zip kiwiMarkdownOutputPath
	zip.output_file = "#{@env_buildnameKiwiMarkdown}.zip"
	zip.output_path = @env_buildfolderpath
end

zip :zipKiwiMvc3 do |zip|
	zip.directories_to_zip kiwiMvc3OutputPath
	zip.output_file = "#{@env_buildnameKiwiMvc3}.zip"
	zip.output_path = @env_buildfolderpath
end

exec :packKiwiMarkdownNuGet do |cmd|
	cmd.command = "NuGet.exe"
	cmd.parameters = "pack #{@env_projectnameKiwiMarkdown}.nuspec -version #{@env_version} -basepath #{kiwiMarkdownOutputPath} -outputdirectory #{@env_buildfolderpath}"
end

exec :packKiwiMvc3NuGet do |cmd|
	cmd.command = "NuGet.exe"
	cmd.parameters = "pack #{@env_projectnameKiwiMvc3}.nuspec -version #{@env_version} -basepath #{@env_solutionfolderpath} -outputdirectory #{@env_buildfolderpath}"
end