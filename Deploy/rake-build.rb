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
@env_projectname = 'Kiwi.Markdown'
@env_solutionfolderpath = "../Source/"
@env_buildversion = "0.3.0" + (ENV['env_buildnumber'].to_s.empty? ? "" : ".#{ENV['env_buildnumber'].to_s}")
@env_buildconfigname = ENV['env_buildconfigname'].to_s.empty? ? "Release" : ENV['env_buildconfigname'].to_s
@env_buildname = "#{@env_solutionname}-v#{@env_buildversion}-#{@env_buildconfigname}"
@env_buildfolderpath = "#{ENV['env_buildfolderpath']}builds/#{@env_buildname}/"
@env_binariesfolderpath = "#{@env_buildfolderpath}binaries"
#--------------------------------------
#optional if no remote nuget actions should be performed
@env_nugetPublishApiKey = ENV['env_nugetPublishApiKey']
@env_nugetPublishUrl = ENV['env_nugetPublishUrl']
@env_nugetSourceUrl = ENV['env_nugetSourceUrl']
#--------------------------------------
# Albacore flow controlling tasks
#--------------------------------------
task :ci => [:buildIt, :testIt, :packageIt, :deployIt]

task :local => [:buildIt, :testIt, :packageIt]
#--------------------------------------
task :buildIt => [:versionIt, :compileIt, :copyBinaries]

task :testIt => [:unittests, :specifications]

task :packageIt => [:createZip, :createKiwiMarkdownNuGet]

task :deployIt => [:publishKiwiMarkDownNuGet]
#--------------------------------------
# Albacore tasks
#--------------------------------------
assemblyinfo :versionIt do |asm|
  sharedAssemblyInfoPath = "#{@env_solutionfolderpath}SharedAssemblyInfo.cs"
  
  asm.input_file = sharedAssemblyInfoPath
  asm.output_file = sharedAssemblyInfoPath
  asm.version = @env_buildversion
  asm.file_version = @env_buildversion  
end

task :createCleanBuildFolders do
  FileUtils.rm_rf(@env_buildfolderpath)
  FileUtils.mkdir_p(@env_binariesfolderpath)
end

msbuild :compileIt => [:createCleanBuildFolders] do |msb|
  msb.properties :configuration => @env_buildconfigname
  msb.targets :Clean, :Build
  msb.solution = "#{@env_solutionfolderpath}#{@env_solutionname}.sln"
end

task :copyBinaries do
  FileUtils.cp_r(FileList["#{@env_solutionfolderpath}Projects/#{@env_projectname}/bin/#{@env_buildconfigname}/*.*"], @env_binariesfolderpath)
end

nunit :unittests do |nunit|
  nunit.command = "#{@env_solutionfolderpath}packages/NUnit.2.5.10.11092/tools/nunit-console.exe"
  nunit.options "/framework=v4.0.30319","/xml=#{@env_buildfolderpath}NUnit-Report-#{@env_projectname}-UnitTests.xml"
  nunit.assemblies FileList["#{@env_solutionfolderpath}Tests/#{@env_solutionname}.UnitTests/bin/#{@env_buildconfigname}/#{@env_solutionname}.UnitTests.dll"].exclude(/obj\//)
end

mspec :specifications do |mspec|
  mspec.command = "#{@env_solutionfolderpath}packages/Machine.Specifications.0.5.0.0/tools/mspec-clr4.exe"
  mspec.options "--html #{@env_buildfolderpath}MSpec-Report-#{@env_projectname}.html"
  mspec.assemblies FileList["#{@env_solutionfolderpath}Tests/#{@env_solutionname}.Specifications/bin/#{@env_buildconfigname}/#{@env_solutionname}.Specifications.dll"].exclude(/obj\//)
end

zip :createZip do |zip|
  zip.directories_to_zip "#{@env_binariesfolderpath}"
  zip.output_file = "#{@env_buildname}.zip"
  zip.output_path = @env_buildfolderpath
end

exec :createKiwiMarkdownNuGet do |cmd|
  cmd.command = "NuGet.exe"
  cmd.parameters = "pack #{@env_projectname}.nuspec -version #{@env_buildversion} -basepath #{@env_binariesfolderpath} -outputdirectory #{@env_buildfolderpath}"
end

exec :publishKiwiMarkdownNuGet do |cmd|
  cmd.command = "NuGet.exe"
  cmd.parameters = "push #{@env_buildfolderpath}#{@env_projectname}.#{@env_buildversion}.nupkg #{@env_nugetPublishApiKey} -src #{@env_nugetPublishUrl}"
end